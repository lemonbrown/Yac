import React, { useState } from 'react';
import Sidebar from './Sidebar';
import QueryEditor from '../QueryEditor/QueryEditor';
import ResultsPanel from '../Results/ResultsPanel';
import { QueryResult } from '../../types';
import { Database } from 'lucide-react';
import ThemeToggle from '../UI/ThemeToggle';

const AppLayout: React.FC = () => {
  const [currentQuery, setCurrentQuery] = useState<string>('');
  const [queryResults, setQueryResults] = useState<QueryResult | null>(null);
  const [isExecuting, setIsExecuting] = useState(false);
  const [splitPosition, setSplitPosition] = useState(50);
  
  const handleQueryChange = (query: string) => {
    setCurrentQuery(query);
  };

  const executeQuery = async () => {
    if (!currentQuery.trim()) return;
    
    setIsExecuting(true);
    
    try {
      // In a real app, this would be an API call to execute the query
      // For now, we'll simulate a response with mock data
      await new Promise(resolve => setTimeout(resolve, 800));
      
      const mockResult: QueryResult = {
        columns: ['player_id', 'name', 'team', 'position', 'yards', 'touchdowns'],
        rows: [
          { player_id: 1, name: 'Patrick Mahomes', team: 'KC', position: 'QB', yards: 5250, touchdowns: 41 },
          { player_id: 2, name: 'Justin Jefferson', team: 'MIN', position: 'WR', yards: 1809, touchdowns: 8 },
          { player_id: 3, name: 'Travis Kelce', team: 'KC', position: 'TE', yards: 1338, touchdowns: 12 },
          { player_id: 4, name: 'Christian McCaffrey', team: 'SF', position: 'RB', yards: 1459, touchdowns: 14 },
          { player_id: 5, name: 'Tyreek Hill', team: 'MIA', position: 'WR', yards: 1710, touchdowns: 7 },
        ],
        executionTime: 0.12,
        rowCount: 5
      };
      
      setQueryResults(mockResult);
    } catch (error) {
      console.error('Error executing query:', error);
    } finally {
      setIsExecuting(false);
    }
  };
  
  const handleMouseDown = (e: React.MouseEvent) => {
    e.preventDefault();
    
    const startY = e.clientY;
    const startSplit = splitPosition;
    
    const handleMouseMove = (e: MouseEvent) => {
      const containerHeight = window.innerHeight - 64; // Accounting for header
      const newPosition = startSplit + ((e.clientY - startY) / containerHeight * 100);
      setSplitPosition(Math.min(Math.max(20, newPosition), 80));
    };
    
    const handleMouseUp = () => {
      document.removeEventListener('mousemove', handleMouseMove);
      document.removeEventListener('mouseup', handleMouseUp);
    };
    
    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
  };

  return (
    <div className="flex flex-col h-screen bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100">
      <header className="h-16 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 flex items-center px-4 shadow-sm">
        <div className="flex items-center">
          <Database className="w-6 h-6 text-blue-600 dark:text-blue-400 mr-2" />
          <h1 className="text-xl font-bold">yac-db</h1>
        </div>
        <div className="ml-auto">
          <ThemeToggle />
        </div>
      </header>
      
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        
        <main className="flex-1 flex flex-col overflow-hidden">
          <div 
            className="flex-none" 
            style={{ height: `${splitPosition}%` }}
          >
            <QueryEditor 
              value={currentQuery} 
              onChange={handleQueryChange} 
              onExecute={executeQuery}
              isExecuting={isExecuting}
            />
          </div>
          
          <div 
            className="h-2 bg-gray-200 dark:bg-gray-700 cursor-row-resize flex items-center justify-center"
            onMouseDown={handleMouseDown}
          >
            <div className="w-8 h-1 bg-gray-400 dark:bg-gray-500 rounded-full"></div>
          </div>
          
          <div className="flex-1 overflow-hidden">
            <ResultsPanel results={queryResults} />
          </div>
        </main>
      </div>
    </div>
  );
};

export default AppLayout;