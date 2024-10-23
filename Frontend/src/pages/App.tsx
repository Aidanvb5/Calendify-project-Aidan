import { Routes, Route } from 'react-router-dom';
import Home from './Home'; // Adjust the path if necessary

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      {/* Add other routes here */}
    </Routes>
  );
};

export default App;