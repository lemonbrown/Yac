export interface QueryResult {
  columns: string[];
  rows: Record<string, any>[];
  executionTime: number;
  rowCount: number;
}

export interface SavedQuery {
  id: string;
  name: string;
  query: string;
  createdAt: string;
  isFavorite: boolean;
}

export interface TableSchema {
  name: string;
  columns: {
    name: string;
    type: string;
    isPrimary: boolean;
    isForeign: boolean;
    references?: {
      table: string;
      column: string;
    };
  }[];
}

export type Theme = 'light' | 'dark';