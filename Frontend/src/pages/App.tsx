import { Routes, Route } from 'react-router-dom';
import Home from './Home';
import Login from './Login';
import ErrorPage from '../shared/ErrorPage';

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="*" element={<ErrorPage />} />
    </Routes>
  );
};

export default App;