import React, { useState } from 'react';
import { QueryResult } from '../../types';
import { Download, ArrowUp, ArrowDown, Clock, Database, BarChart2, Table as TableIcon } from 'lucide-react';
import VisualizationPanel from './VisualizationPanel';

interface ResultsPanelProps {
  results: QueryResult | null;
}

const ResultsPanel: React.FC<ResultsPanelProps> = ({ results }) => {
  const [sortColumn, setSortColumn] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [view, setView] = useState<'table' | 'chart'>('table');
  
  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortColumn(column);
      setSortDirection('asc');
    }
  };
  
  const sortedRows = results?.rows ? [...results.rows].sort((a, b) => {
    if (!sortColumn) return 0;
    
    const aValue = a[sortColumn];
    const bValue = b[sortColumn];
    
    if (typeof aValue === 'number' && typeof bValue === 'number') {
      return sortDirection === 'asc' ? aValue - bValue : bValue - aValue;
    }
    
    const strA = String(aValue || '');
    const strB = String(bValue || '');
    
    return sortDirection === 'asc' 
      ? strA.localeCompare(strB) 
      : strB.localeCompare(strA);
  }) : [];
  
  const exportData = (format: 'csv' | 'json') => {
    if (!results) return;
    
    let content: string;
    let fileName: string;
    let mimeType: string;
    
    if (format === 'csv') {
      const headers = results.columns.join(',');
      const rows = results.rows.map(row => {
        return results.columns.map(col => {
          const value = row[col];
          return typeof value === 'string' && value.includes(',') 
            ? `"${value}"` 
            : value;
        }).join(',');
      }).join('\n');
      
      content = `${headers}\n${rows}`;
      fileName = `nfl_query_result_${new Date().toISOString().split('T')[0]}.csv`;
      mimeType = 'text/csv';
    } else {
      content = JSON.stringify(results.rows, null, 2);
      fileName = `nfl_query_result_${new Date().toISOString().split('T')[0]}.json`;
      mimeType = 'application/json';
    }
    
    const blob = new Blob([content], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
  };
  
  if (!results) {
    return (
      <div className="flex flex-col items-center justify-center h-full bg-white dark:bg-gray-800 text-gray-500 dark:text-gray-400">
        <Database size={48} className="mb-4 opacity-30" />
        <p>Execute a query to see results</p>
      </div>
    );
  }
  
  return (
    <div className="flex flex-col h-full bg-white dark:bg-gray-800">
      <div className="flex items-center justify-between p-2 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
        <div className="flex items-center text-sm text-gray-600 dark:text-gray-400">
          <Clock size={14} className="mr-1" />
          <span>{results.executionTime.toFixed(2)}s</span>
          <span className="mx-2">•</span>
          <span>{results.rowCount} rows</span>
        </div>
        <div className="flex items-center space-x-2">
          <div className="border-r border-gray-200 dark:border-gray-700 pr-2 mr-2">
            <button
              className={`p-1.5 rounded text-sm flex items-center ${
                view === 'table' 
                  ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400' 
                  : 'hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-300'
              }`}
              onClick={() => setView('table')}
              title="Table view"
            >
              <TableIcon size={14} className="mr-1" />
              <span>Table</span>
            </button>
          </div>
          <button
            className={`p-1.5 rounded text-sm flex items-center ${
              view === 'chart' 
                ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400' 
                : 'hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-300'
            }`}
            onClick={() => setView('chart')}
            title="Chart view"
          >
            <BarChart2 size={14} className="mr-1" />
            <span>Chart</span>
          </button>
          <div className="border-l border-gray-200 dark:border-gray-700 pl-2 ml-2">
            <button 
              className="p-1.5 rounded text-sm flex items-center hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-300"
              onClick={() => exportData('csv')}
              title="Export as CSV"
            >
              <Download size={14} className="mr-1" />
              <span>CSV</span>
            </button>
            <button 
              className="p-1.5 rounded text-sm flex items-center hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-300"
              onClick={() => exportData('json')}
              title="Export as JSON"
            >
              <Download size={14} className="mr-1" />
              <span>JSON</span>
            </button>
          </div>
        </div>
      </div>
      
      <div className="flex-1 overflow-auto">
        {view === 'table' ? (
          <table className="min-w-full border-collapse">
            <thead className="bg-gray-100 dark:bg-gray-850 sticky top-0">
              <tr>
                {results.columns.map((column) => (
                  <th 
                    key={column}
                    className="px-4 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider border-b border-gray-200 dark:border-gray-700 hover:bg-gray-200 dark:hover:bg-gray-700 cursor-pointer select-none"
                    onClick={() => handleSort(column)}
                  >
                    <div className="flex items-center">
                      <span>{column}</span>
                      {sortColumn === column && (
                        <span className="ml-1">
                          {sortDirection === 'asc' 
                            ? <ArrowUp size={14} /> 
                            : <ArrowDown size={14} />
                          }
                        </span>
                      )}
                    </div>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              {sortedRows.map((row, rowIdx) => (
                <tr 
                  key={rowIdx} 
                  className="hover:bg-gray-50 dark:hover:bg-gray-750 transition-colors duration-150"
                >
                  {results.columns.map((column) => (
                    <td 
                      key={`${rowIdx}-${column}`}
                      className="px-4 py-2 text-sm text-gray-900 dark:text-gray-300 font-mono"
                    >
                      {row[column] !== null && row[column] !== undefined 
                        ? String(row[column]) 
                        : <span className="text-gray-400 dark:text-gray-600">NULL</span>
                      }
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <VisualizationPanel data={results} />
        )}
      </div>
    </div>
  );
};

export default ResultsPanel;