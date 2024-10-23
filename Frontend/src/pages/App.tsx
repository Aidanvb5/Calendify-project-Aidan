import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home'; // Ensure this path is correct

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        {/* Define other routes here */}
      </Routes>
    </Router>
  );
};

export default App;