import React, { useState } from 'react';

interface LoginBody {
  Username: string;
  Password: string;
}

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault(); // Prevent the default form submission

    const response = await fetch('http://localhost:<port>/api/v1/Login/Login', { // Replace <port> with your backend port
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ Username: username, Password: password }) as string,
    });

    if (response.ok) {
      const data = await response.json();
      setMessage(data.message); // Display success message
      // Optionally, you can redirect or perform other actions here
    } else {
      const errorMessage = await response.text();
      setMessage(errorMessage); // Display error message
    }
  };

  return (
    <div className="container px-5 my-5">
      <h1>Login</h1>
      <form onSubmit={handleLogin}>
        <div className="mb-3">
          <label htmlFor="username" className="form-label">Username</label>
          <input
            type="text"
            className="form-control"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div className="mb-3">
          <label htmlFor="password" className="form-label">Password</label>
          <input
            type="password"
            className="form-control"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit" className="btn btn-primary">Login</button>
      </form>
      {message && <div className="mt-3">{message}</div>}
    </div>
  );
};

export default Login;