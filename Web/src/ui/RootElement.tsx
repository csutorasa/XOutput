import React from 'react';
import green from '@mui/material/colors/green';
import { HashRouter } from 'react-router-dom';
import { Router } from './Router';
import { createTheme, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from 'react-query';

const productionTheme = createTheme({});

const developmentTheme = createTheme({
  palette: {
    primary: green,
  },
});

const theme = process.env.NODE_ENV !== 'production' ? developmentTheme : productionTheme;

const queryClient = new QueryClient();

export const RootElement = (
  <HashRouter>
    <ThemeProvider theme={theme}>
      <QueryClientProvider client={queryClient}>
        <Router></Router>
      </QueryClientProvider>
    </ThemeProvider>
  </HashRouter>
);
export default RootElement;
