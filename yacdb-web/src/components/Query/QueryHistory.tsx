import React from 'react';
import { Clock, ChevronRight, Play, Trash } from 'lucide-react';

const QueryHistory: React.FC = () => {
  // Mock query history data
  const mockHistory = [
    {
      id: '1',
      query: 'SELECT player_id, name, team, position FROM players WHERE position = "QB" ORDER BY name',
      timestamp: '2025-04-12T15:45:00Z',
      executionTime: 0.12
    },
    {
      id: '2',
      query: 'SELECT team, COUNT(*) as player_count FROM players GROUP BY team ORDER BY player_count DESC',
      timestamp: '2025-04-12T15:42:30Z',
      executionTime: 0.18
    },
    {
      id: '3',
      query: 'SELECT p.name, t.name as team_name, p.position FROM players p JOIN teams t ON p.team_id = t.team_id WHERE t.division = "NFC East"',
      timestamp: '2025-04-12T15:40:00Z',
      executionTime: 0.25
    },
    {
      id: '4',
      query: 'SELECT ps.player_id, p.name, SUM(ps.receiving_yards) as total_yards FROM player_stats ps JOIN players p ON ps.player_id = p.player_id GROUP BY ps.player_id ORDER BY total_yards DESC LIMIT 10',
      timestamp: '2025-04-12T15:35:00Z',
      executionTime: 0.32
    },
  ];
  
  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };
  
  return (
    <div className="p-2">
      <div className="flex items-center mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">
        <Clock size={16} className="mr-2" />
        <span>Recent Queries</span>
      </div>
      
      <div className="space-y-2">
        {mockHistory.map((query) => (
          <div 
            key={query.id}
            className="text-sm border border-gray-200 dark:border-gray-700 rounded hover:bg-gray-50 dark:hover:bg-gray-750 group"
          >
            <div className="p-2 cursor-pointer flex items-center">
              <ChevronRight size={16} className="text-gray-400 flex-shrink-0" />
              <div className="ml-1 flex-1 overflow-hidden">
                <div className="font-mono text-xs truncate">{query.query}</div>
                <div className="flex items-center mt-1 text-xs text-gray-500 dark:text-gray-400">
                  <span>{formatTime(query.timestamp)}</span>
                  <span className="mx-1">•</span>
                  <span>{query.executionTime.toFixed(2)}s</span>
                </div>
              </div>
            </div>
            
            <div className="border-t border-gray-200 dark:border-gray-700 flex">
              <button 
                className="flex items-center justify-center py-1 flex-1 text-xs text-blue-600 dark:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20"
                title="Run query again"
              >
                <Play size={12} className="mr-1" />
                <span>Run</span>
              </button>
              
              <div className="w-px bg-gray-200 dark:bg-gray-700"></div>
              
              <button 
                className="flex items-center justify-center py-1 flex-1 text-xs text-red-500 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20"
                title="Remove from history"
              >
                <Trash size={12} className="mr-1" />
                <span>Delete</span>
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default QueryHistory;