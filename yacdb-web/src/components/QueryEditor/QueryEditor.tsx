import React, { useRef, useEffect } from 'react';
import { Play, Save, Copy, Trash } from 'lucide-react';

interface QueryEditorProps {
  value: string;
  onChange: (value: string) => void;
  onExecute: () => void;
  isExecuting: boolean;
}

const QueryEditor: React.FC<QueryEditorProps> = ({ 
  value, 
  onChange, 
  onExecute,
  isExecuting
}) => {
  const editorRef = useRef<HTMLTextAreaElement>(null);
  
  useEffect(() => {
    // Set focus on the editor when component mounts
    if (editorRef.current) {
      editorRef.current.focus();
    }
  }, []);
  
  const handleKeyDown = (e: React.KeyboardEvent) => {
    // Execute query on Ctrl+Enter or Cmd+Enter
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
      e.preventDefault();
      onExecute();
    }
    
    // Handle tab key for indentation
    if (e.key === 'Tab') {
      e.preventDefault();
      const start = editorRef.current?.selectionStart || 0;
      const end = editorRef.current?.selectionEnd || 0;
      
      const newValue = value.substring(0, start) + '  ' + value.substring(end);
      onChange(newValue);
      
      // Set cursor position after the inserted tab
      setTimeout(() => {
        if (editorRef.current) {
          editorRef.current.selectionStart = editorRef.current.selectionEnd = start + 2;
        }
      }, 0);
    }
  };
  
  const clearQuery = () => {
    onChange('');
    if (editorRef.current) {
      editorRef.current.focus();
    }
  };
  
  const copyQuery = () => {
    navigator.clipboard.writeText(value);
  };
  
  return (
    <div className="flex flex-col h-full bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
      <div className="flex items-center justify-between p-2 border-b border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
        <div className="text-sm font-medium">YQL Query</div>
        <div className="flex space-x-2">
          <button 
            className="p-1.5 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-500 dark:text-gray-400"
            onClick={clearQuery}
            title="Clear query"
          >
            <Trash size={16} />
          </button>
          <button 
            className="p-1.5 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-500 dark:text-gray-400"
            onClick={copyQuery}
            title="Copy query"
          >
            <Copy size={16} />
          </button>
          <button 
            className="p-1.5 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-500 dark:text-gray-400"
            title="Save query"
          >
            <Save size={16} />
          </button>
          <button 
            className={`p-1.5 rounded font-medium flex items-center ${
              isExecuting 
                ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 cursor-not-allowed' 
                : 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 hover:bg-blue-200 dark:hover:bg-blue-800/40'
            }`}
            onClick={onExecute}
            disabled={isExecuting}
            title="Execute query (Ctrl+Enter)"
          >
            <Play size={16} className="mr-1" />
            <span>Run</span>
          </button>
        </div>
      </div>
      
      <div className="flex-1 p-0 overflow-hidden">
        <textarea
          ref={editorRef}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          className="w-full h-full p-4 resize-none font-mono text-sm bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 outline-none"
          placeholder="-- Write your YQL query here"
          spellCheck={false}
        />
      </div>
    </div>
  );
};

export default QueryEditor;