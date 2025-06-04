import React, { useState } from 'react';
import { ChevronRight, ChevronDown, Database, Table, Key, ExternalLink } from 'lucide-react';
import { TableSchema } from '../../types';

const SchemaExplorer: React.FC = () => {
  const [expandedTables, setExpandedTables] = useState<string[]>([]);
  
  // Mock NFL database schema
  const mockSchema: TableSchema[] = [
    {
      name: 'players',
      columns: [
        { name: 'player_id', type: 'INTEGER', isPrimary: true, isForeign: false },
        { name: 'name', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
        { name: 'team_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'teams', column: 'team_id' } },
        { name: 'position', type: 'VARCHAR(10)', isPrimary: false, isForeign: false },
        { name: 'jersey_number', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'height', type: 'VARCHAR(10)', isPrimary: false, isForeign: false },
        { name: 'weight', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'birth_date', type: 'DATE', isPrimary: false, isForeign: false },
        { name: 'college', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
      ]
    },
    {
      name: 'teams',
      columns: [
        { name: 'team_id', type: 'INTEGER', isPrimary: true, isForeign: false },
        { name: 'name', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
        { name: 'city', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
        { name: 'abbreviation', type: 'VARCHAR(10)', isPrimary: false, isForeign: false },
        { name: 'conference', type: 'VARCHAR(10)', isPrimary: false, isForeign: false },
        { name: 'division', type: 'VARCHAR(10)', isPrimary: false, isForeign: false },
        { name: 'stadium_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'stadiums', column: 'stadium_id' } },
      ]
    },
    {
      name: 'games',
      columns: [
        { name: 'game_id', type: 'INTEGER', isPrimary: true, isForeign: false },
        { name: 'season', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'week', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'home_team_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'teams', column: 'team_id' } },
        { name: 'away_team_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'teams', column: 'team_id' } },
        { name: 'home_score', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'away_score', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'game_date', type: 'DATE', isPrimary: false, isForeign: false },
        { name: 'stadium_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'stadiums', column: 'stadium_id' } },
      ]
    },
    {
      name: 'player_stats',
      columns: [
        { name: 'stat_id', type: 'INTEGER', isPrimary: true, isForeign: false },
        { name: 'player_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'players', column: 'player_id' } },
        { name: 'game_id', type: 'INTEGER', isPrimary: false, isForeign: true, references: { table: 'games', column: 'game_id' } },
        { name: 'passing_yards', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'passing_tds', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'rushing_yards', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'rushing_tds', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'receiving_yards', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'receiving_tds', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'tackles', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'sacks', type: 'DECIMAL(3,1)', isPrimary: false, isForeign: false },
        { name: 'interceptions', type: 'INTEGER', isPrimary: false, isForeign: false },
      ]
    },
    {
      name: 'stadiums',
      columns: [
        { name: 'stadium_id', type: 'INTEGER', isPrimary: true, isForeign: false },
        { name: 'name', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
        { name: 'city', type: 'VARCHAR(100)', isPrimary: false, isForeign: false },
        { name: 'state', type: 'VARCHAR(2)', isPrimary: false, isForeign: false },
        { name: 'capacity', type: 'INTEGER', isPrimary: false, isForeign: false },
        { name: 'surface_type', type: 'VARCHAR(50)', isPrimary: false, isForeign: false },
        { name: 'roof_type', type: 'VARCHAR(50)', isPrimary: false, isForeign: false },
      ]
    },
  ];
  
  const toggleTable = (tableName: string) => {
    setExpandedTables(prev => 
      prev.includes(tableName) 
        ? prev.filter(t => t !== tableName) 
        : [...prev, tableName]
    );
  };
  
  const handleCopyTableName = (tableName: string) => {
    navigator.clipboard.writeText(tableName);
  };
  
  return (
    <div className="p-2">
      <div className="flex items-center mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">
        <Database size={16} className="mr-2" />
        <span>NFL Database</span>
      </div>
      
      <div className="space-y-1">
        {mockSchema.map((table) => (
          <div key={table.name} className="text-sm">
            <div 
              className="flex items-center py-1 px-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded cursor-pointer"
              onClick={() => toggleTable(table.name)}
            >
              {expandedTables.includes(table.name) ? (
                <ChevronDown size={16} className="text-gray-500 dark:text-gray-400" />
              ) : (
                <ChevronRight size={16} className="text-gray-500 dark:text-gray-400" />
              )}
              <Table size={14} className="ml-1 mr-2 text-blue-600 dark:text-blue-400" />
              <span className="font-medium">{table.name}</span>
              <button
                className="ml-auto opacity-0 group-hover:opacity-100 hover:bg-gray-200 dark:hover:bg-gray-600 p-0.5 rounded"
                onClick={(e) => {
                  e.stopPropagation();
                  handleCopyTableName(table.name);
                }}
                title="Copy table name"
              >
                <span className="text-xs text-gray-500 dark:text-gray-400">Copy</span>
              </button>
            </div>
            
            {expandedTables.includes(table.name) && (
              <div className="ml-6 pl-2 border-l border-gray-200 dark:border-gray-700 space-y-1 py-1">
                {table.columns.map((column) => (
                  <div 
                    key={`${table.name}-${column.name}`}
                    className="flex items-center py-0.5 px-2 hover:bg-gray-100 dark:hover:bg-gray-700 rounded text-xs group"
                  >
                    {column.isPrimary && (
                      <Key size={12} className="mr-1 text-amber-500" title="Primary Key" />
                    )}
                    {column.isForeign && (
                      <ExternalLink size={12} className="mr-1 text-purple-500" title="Foreign Key" />
                    )}
                    <span className="font-medium mr-2">{column.name}</span>
                    <span className="text-gray-500 dark:text-gray-400">{column.type}</span>
                    
                    {column.isForeign && column.references && (
                      <span className="ml-2 text-purple-500 dark:text-purple-400">
                        → {column.references.table}.{column.references.column}
                      </span>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default SchemaExplorer;