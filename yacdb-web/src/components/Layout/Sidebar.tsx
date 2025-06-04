import React, { useState } from 'react';
import { History, Star, Database, Save, Layers } from 'lucide-react';
import SchemaExplorer from '../Schema/SchemaExplorer';
import QueryHistory from '../Query/QueryHistory';
import SavedQueries from '../Query/SavedQueries';

type Tab = 'schema' | 'history' | 'saved';

const Sidebar: React.FC = () => {
  const [activeTab, setActiveTab] = useState<Tab>('schema');
  const [isExpanded, setIsExpanded] = useState(true);
  
  const renderTabContent = () => {
    switch (activeTab) {
      case 'schema':
        return <SchemaExplorer />;
      case 'history':
        return <QueryHistory />;
      case 'saved':
        return <SavedQueries />;
      default:
        return null;
    }
  };
  
  return (
    <div className={`flex flex-col border-r border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 transition-all duration-300 ${isExpanded ? 'w-64' : 'w-16'}`}>
      <div className="flex items-center justify-between p-3 border-b border-gray-200 dark:border-gray-700">
        <h2 className={`font-semibold ${isExpanded ? 'block' : 'hidden'}`}>Explorer</h2>
        <button 
          onClick={() => setIsExpanded(!isExpanded)}
          className="p-1 rounded hover:bg-gray-100 dark:hover:bg-gray-700"
        >
          <Layers size={18} className="text-gray-500 dark:text-gray-400" />
        </button>
      </div>
      
      <div className="flex border-b border-gray-200 dark:border-gray-700">
        <button
          className={`flex items-center justify-center py-3 flex-1 transition-colors duration-200 ${activeTab === 'schema' ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400' : 'text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700'}`}
          onClick={() => setActiveTab('schema')}
          title="Database Schema"
        >
          <Database size={18} />
          {isExpanded && <span className="ml-2">Schema</span>}
        </button>
        
        <button
          className={`flex items-center justify-center py-3 flex-1 transition-colors duration-200 ${activeTab === 'history' ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400' : 'text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700'}`}
          onClick={() => setActiveTab('history')}
          title="Query History"
        >
          <History size={18} />
          {isExpanded && <span className="ml-2">History</span>}
        </button>
        
        <button
          className={`flex items-center justify-center py-3 flex-1 transition-colors duration-200 ${activeTab === 'saved' ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400' : 'text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700'}`}
          onClick={() => setActiveTab('saved')}
          title="Saved Queries"
        >
          <Star size={18} />
          {isExpanded && <span className="ml-2">Saved</span>}
        </button>
      </div>
      
      <div className={`flex-1 overflow-y-auto ${isExpanded ? 'block' : 'hidden'}`}>
        {renderTabContent()}
      </div>
    </div>
  );
};

export default Sidebar;