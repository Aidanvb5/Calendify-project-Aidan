import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

const EventDetails: React.FC = () => {
    const { eventId } = useParams<{ eventId: string }>();
    const [event, setEvent] = useState<any>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchEventDetails = async () => {
            try {
                const response = await fetch(`http://localhost:<port>/api/Events/${eventId}`, {
                    method: 'GET',
                    credentials: 'include',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch event details');
                }

                const data = await response.json();
                setEvent(data);
            } catch (error) {
                setError('Error fetching event details');
                console.error('Error fetching event details:', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchEventDetails();
    }, [eventId]);

    if (isLoading) {
        return <div>Loading event details...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    return (
        <div className="event-details">
            <h1>{event.title}</h1>
            <p>{event.description}</p>
            <p>Date: {event.date}</p>
            <p>Time: {event.startTime} - {event.endTime}</p>
            <p>Location: {event.location}</p>
            <h2>Reviews</h2>
            <ul>
                {event.reviews.map((review: any) => (
                    <li key={review.id}>
                        <strong>{review.comment}</strong> - Rating: {review.rating}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default EventDetails;
