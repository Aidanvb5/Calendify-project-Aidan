import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Home from './Home';
import Login from './Login';
import UserDashboard from './UserDashboard';
import AdminDashboard from './AdminDashboard';
import EventDetails from './EventDetails';
import ErrorPage from '../shared/ErrorPage';
import { useAuth } from '../context/AuthContext';

const App: React.FC = () => {
    const { isAuthenticated } = useAuth();

    return (
        <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />

            {/* Protected User Routes */}
            <Route 
                path="/user/dashboard" 
                element={isAuthenticated ? <UserDashboard /> : <Navigate to="/login" />} 
            />

            {/* Protected Admin Routes */}
            <Route 
                path="/admin/dashboard" 
                element={isAuthenticated ? <AdminDashboard /> : <Navigate to="/login" />} 
            />

            {/* Event Details Route */}
            <Route 
                path="/events/:eventId" 
                element={isAuthenticated ? <EventDetails /> : <Navigate to="/login" />} 
            />

            {/* Catch-all Error Route */}
            <Route path="*" element={<ErrorPage />} />
        </Routes>
    );
};

export default App;
