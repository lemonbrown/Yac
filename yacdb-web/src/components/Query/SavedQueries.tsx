import React from 'react';
import { Star, ChevronRight, Play, Edit, Trash } from 'lucide-react';
import { SavedQuery } from '../../types';

const SavedQueries: React.FC = () => {
  // Mock saved queries data
  const mockSavedQueries: SavedQuery[] = [
    {
      id: '1',
      name: 'Top Quarterbacks by Passing Yards',
      query: 'SELECT p.name, t.name as team, SUM(ps.passing_yards) as total_yards FROM player_stats ps JOIN players p ON ps.player_id = p.player_id JOIN teams t ON p.team_id = t.team_id WHERE p.position = "QB" GROUP BY ps.player_id ORDER BY total_yards DESC LIMIT 10',
      createdAt: '2025-04-10T10:30:00Z',
      isFavorite: true
    },
    {
      id: '2',
      name: 'Team Roster with Player Details',
      query: 'SELECT p.name, p.position, p.jersey_number, p.height, p.weight, p.college FROM players p JOIN teams t ON p.team_id = t.team_id WHERE t.abbreviation = "KC" ORDER BY p.position, p.jersey_number',
      createdAt: '2025-04-09T14:15:00Z',
      isFavorite: true
    },
    {
      id: '3',
      name: 'Game Results by Week',
      query: 'SELECT g.week, ht.name as home_team, at.name as away_team, g.home_score, g.away_score, g.game_date, s.name as stadium FROM games g JOIN teams ht ON g.home_team_id = ht.team_id JOIN teams at ON g.away_team_id = at.team_id JOIN stadiums s ON g.stadium_id = s.stadium_id WHERE g.season = 2024 ORDER BY g.week, g.game_date',
      createdAt: '2025-04-08T09:45:00Z',
      isFavorite: false
    },
  ];
  
  const formatDate = (timestamp: string) => {
    const date = new Date(timestamp);
    return date.toLocaleDateString();
  };
  
  return (
    <div className="p-2">
      <div className="flex items-center mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">
        <Star size={16} className="mr-2" />
        <span>Saved Queries</span>
      </div>
      
      <div className="space-y-2">
        {mockSavedQueries.map((query) => (
          <div 
            key={query.id}
            className="text-sm border border-gray-200 dark:border-gray-700 rounded hover:bg-gray-50 dark:hover:bg-gray-750 group"
          >
            <div className="p-2 cursor-pointer flex items-center">
              <ChevronRight size={16} className="text-gray-400 flex-shrink-0" />
              <div className="ml-1 flex-1 overflow-hidden">
                <div className="flex items-center">
                  <span className="font-medium">{query.name}</span>
                  {query.isFavorite && (
                    <Star size={14} className="ml-1 text-yellow-500 fill-yellow-500" />
                  )}
                </div>
                <div className="font-mono text-xs text-gray-600 dark:text-gray-400 truncate mt-1">
                  {query.query}
                </div>
                <div className="text-xs text-gray-500 dark:text-gray-400 mt-1">
                  {formatDate(query.createdAt)}
                </div>
              </div>
            </div>
            
            <div className="border-t border-gray-200 dark:border-gray-700 flex">
              <button 
                className="flex items-center justify-center py-1 flex-1 text-xs text-blue-600 dark:text-blue-400 hover:bg-blue-50 dark:hover:bg-blue-900/20"
                title="Run query"
              >
                <Play size={12} className="mr-1" />
                <span>Run</span>
              </button>
              
              <div className="w-px bg-gray-200 dark:bg-gray-700"></div>
              
              <button 
                className="flex items-center justify-center py-1 flex-1 text-xs text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700"
                title="Edit query"
              >
                <Edit size={12} className="mr-1" />
                <span>Edit</span>
              </button>
              
              <div className="w-px bg-gray-200 dark:bg-gray-700"></div>
              
              <button 
                className="flex items-center justify-center py-1 flex-1 text-xs text-red-500 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20"
                title="Delete query"
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

export default SavedQueries;