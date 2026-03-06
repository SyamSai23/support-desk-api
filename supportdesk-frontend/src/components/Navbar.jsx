import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const { email, role, logout, token } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <nav className="bg-slate-800 text-white px-6 py-4 flex justify-between items-center">
      <div className="flex gap-4 items-center">
        <Link to="/" className="font-bold text-lg">SupportDesk</Link>

        {token && (
          <>
            <Link to="/tickets">Tickets</Link>
            <Link to="/tickets/new">Create Ticket</Link>
            {(role === "Agent" || role === "Admin") && (
              <Link to="/assigned-to-me">Assigned To Me</Link>
            )}
            {role === "Admin" && <Link to="/admin/create-user">Create User</Link>}
          </>
        )}
      </div>

      <div className="flex gap-4 items-center">
        {token ? (
          <>
            <span className="text-sm">{email} ({role})</span>
            <button
              onClick={handleLogout}
              className="bg-red-500 px-3 py-1 rounded"
            >
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}