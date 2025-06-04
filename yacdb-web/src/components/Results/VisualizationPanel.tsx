import React, { useState, useMemo } from 'react';
import { QueryResult } from '../../types';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
} from 'chart.js';
import { Bar, Line, Pie } from 'react-chartjs-2';
import { BarChart2, LineChart, PieChart } from 'lucide-react';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
);

interface VisualizationPanelProps {
  data: QueryResult;
}

type ChartType = 'bar' | 'line' | 'pie';

const VisualizationPanel: React.FC<VisualizationPanelProps> = ({ data }) => {
  const [chartType, setChartType] = useState<ChartType>('bar');
  const [xAxis, setXAxis] = useState<string>(data.columns[0]);
  const [yAxis, setYAxis] = useState<string>(
    data.columns.find(col => typeof data.rows[0][col] === 'number') || data.columns[1]
  );

  const chartData = useMemo(() => {
    const labels = data.rows.map(row => String(row[xAxis]));
    const values = data.rows.map(row => Number(row[yAxis]));
    
    const backgroundColor = [
      'rgba(54, 162, 235, 0.5)',
      'rgba(255, 99, 132, 0.5)',
      'rgba(75, 192, 192, 0.5)',
      'rgba(255, 206, 86, 0.5)',
      'rgba(153, 102, 255, 0.5)',
    ];
    
    const borderColor = backgroundColor.map(color => color.replace('0.5', '1'));
    
    return {
      labels,
      datasets: [
        {
          label: yAxis,
          data: values,
          backgroundColor: chartType === 'pie' ? backgroundColor : backgroundColor[0],
          borderColor: chartType === 'pie' ? borderColor : borderColor[0],
          borderWidth: 1,
        },
      ],
    };
  }, [data, xAxis, yAxis, chartType]);

  const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
      },
      title: {
        display: true,
        text: `${yAxis} by ${xAxis}`,
      },
    },
  };

  const numericColumns = data.columns.filter(
    col => typeof data.rows[0][col] === 'number'
  );

  return (
    <div className="h-full flex flex-col p-4">
      <div className="flex items-center space-x-4 mb-4">
        <div className="flex items-center space-x-2">
          <button
            className={`p-2 rounded ${
              chartType === 'bar'
                ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400'
                : 'hover:bg-gray-100 dark:hover:bg-gray-700'
            }`}
            onClick={() => setChartType('bar')}
            title="Bar Chart"
          >
            <BarChart2 size={20} />
          </button>
          <button
            className={`p-2 rounded ${
              chartType === 'line'
                ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400'
                : 'hover:bg-gray-100 dark:hover:bg-gray-700'
            }`}
            onClick={() => setChartType('line')}
            title="Line Chart"
          >
            <LineChart size={20} />
          </button>
          <button
            className={`p-2 rounded ${
              chartType === 'pie'
                ? 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400'
                : 'hover:bg-gray-100 dark:hover:bg-gray-700'
            }`}
            onClick={() => setChartType('pie')}
            title="Pie Chart"
          >
            <PieChart size={20} />
          </button>
        </div>

        <div className="flex items-center space-x-4">
          <div className="flex flex-col">
            <label className="text-sm text-gray-600 dark:text-gray-400 mb-1">
              X Axis
            </label>
            <select
              value={xAxis}
              onChange={(e) => setXAxis(e.target.value)}
              className="px-2 py-1 rounded border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-sm"
            >
              {data.columns.map((col) => (
                <option key={col} value={col}>
                  {col}
                </option>
              ))}
            </select>
          </div>

          <div className="flex flex-col">
            <label className="text-sm text-gray-600 dark:text-gray-400 mb-1">
              Y Axis
            </label>
            <select
              value={yAxis}
              onChange={(e) => setYAxis(e.target.value)}
              className="px-2 py-1 rounded border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-sm"
            >
              {numericColumns.map((col) => (
                <option key={col} value={col}>
                  {col}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <div className="flex-1 min-h-0">
        {chartType === 'bar' && <Bar data={chartData} options={chartOptions} />}
        {chartType === 'line' && <Line data={chartData} options={chartOptions} />}
        {chartType === 'pie' && <Pie data={chartData} options={chartOptions} />}
      </div>
    </div>
  );
};

export default VisualizationPanel;