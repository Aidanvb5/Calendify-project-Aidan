import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface Event {
    id: number;
    title: string;
    date: string; // Adjust type as necessary
    startTime: string; // Adjust type as necessary
}

const Home: React.FC = () => {
    const { isAuthenticated } = useAuth();
    const navigate = useNavigate();
    const [events, setEvents] = useState<Event[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
            return;
        }

        const fetchEvents = async () => {
            try {
                const response = await fetch('http://localhost:5000/api/Events', {
                    method: 'GET',
                    credentials: 'include',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch events');
                }

                const data = await response.json();
                setEvents(data);
            } catch (error) {
                setError('Error fetching events');
                console.error('Error fetching events:', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchEvents();
    }, [isAuthenticated, navigate]);

    if (isLoading) {
        return <div>Loading events...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <div className="container px-5 my-5">
            <h1>Upcoming Events</h1>
            <ul>
                {events.map(event => (
                    <li key={event.id}>
                        <Link to={`/events/${event.id}`}>
                            {event.title} - {event.date} at {event.startTime}
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Home;
