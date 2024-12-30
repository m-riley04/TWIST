import { BrowserRouter, Route, Routes } from 'react-router-dom';
import ParticipantLoginPage from './pages/ParticipantLoginPage/ParticipantLoginPage';
import InstructorPage from './pages/InstructorPage/InstructorPage';
import CreateSimulationPage from './pages/CreateSimulationPage/CreateSimulationPage';
import InstructorSimulationRoomPage from './pages/InstructorSimulationRoomPage/InstructorSimulationRoomPage';
import NotFoundPage from './pages/NotFoundPage/NotFoundPage';
import './App.scss';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<ParticipantLoginPage />} index />
                <Route path="instructor" element={<InstructorPage />}/>
                <Route path="instructor/create" element={<CreateSimulationPage />} />
                <Route path="instructor/room/:code" element={<InstructorSimulationRoomPage />} />
                <Route path="*" element={<NotFoundPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;