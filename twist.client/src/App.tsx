import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import ParticipantLoginPage from './pages/ParticipantLoginPage/ParticipantLoginPage';
import InstructorPage from './pages/InstructorPage/InstructorPage';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<ParticipantLoginPage />} index />
                <Route path="instructor" element={<InstructorPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;