import { Routes, Route, Navigate } from 'react-router-dom';
import { useState } from 'react';
import Home from './Home';
import Login from './Login';
import ErrorPage from '../shared/ErrorPage';
import UserDashboard from './UserDashboard';
import AdminDashboard from './AdminDashboard';

const App = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />

      {/* Protected User Routes */}
      <Route path="/user/dashboard" element={<UserDashboard />} />

      {/* Protected Admin Routes */}
      <Route path="/admin/dashboard" element={<AdminDashboard />} />

      {/* Catch-all Error Route */}
      <Route path="*" element={<ErrorPage />} />
    </Routes>
  );
};

export default App;