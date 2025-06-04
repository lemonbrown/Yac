import React, { useEffect, useState } from 'react';
import { Sun, Moon } from 'lucide-react';
import { Theme } from '../../types';

const ThemeToggle: React.FC = () => {
  const [theme, setTheme] = useState<Theme>('light');
  
  useEffect(() => {
    // Check if user has a preferred theme in localStorage
    const savedTheme = localStorage.getItem('nfl-query-theme') as Theme | null;
    
    // If not, check system preference
    if (!savedTheme) {
      const systemPreference = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
      setTheme(systemPreference);
    } else {
      setTheme(savedTheme);
    }
  }, []);
  
  useEffect(() => {
    // Update document class when theme changes
    if (theme === 'dark') {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
    
    // Save preference to localStorage
    localStorage.setItem('nfl-query-theme', theme);
  }, [theme]);
  
  const toggleTheme = () => {
    setTheme(theme === 'light' ? 'dark' : 'light');
  };
  
  return (
    <button
      onClick={toggleTheme}
      className="p-2 rounded-full hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
      title={`Switch to ${theme === 'light' ? 'dark' : 'light'} mode`}
    >
      {theme === 'light' ? (
        <Moon size={20} className="text-gray-700" />
      ) : (
        <Sun size={20} className="text-yellow-300" />
      )}
    </button>
  );
};

export default ThemeToggle;