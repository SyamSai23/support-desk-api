import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../api/axios";
import { useAuth } from "../context/AuthContext";

export default function AssignedTicketsPage() {
  const { role } = useAuth();
  const [tickets, setTickets] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchAssigned = async () => {
      try {
        const res = await api.get("/tickets/assigned-to-me");
        setTickets(res.data.items || []);
      } catch {
        setError("Failed to load assigned tickets");
      }
    };

    if (role === "Agent" || role === "Admin") {
      fetchAssigned();
    }
  }, [role]);

  if (role !== "Agent" && role !== "Admin") {
    return <div className="max-w-4xl mx-auto mt-8 text-red-500">Access denied.</div>;
  }

  return (
    <div className="max-w-5xl mx-auto mt-8 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Assigned To Me</h2>

      {error && <p className="text-red-500">{error}</p>}

      <div className="space-y-4">
        {tickets.map((ticket) => (
          <Link
            key={ticket.id}
            to={`/tickets/${ticket.id}`}
            className="block border p-4 rounded hover:bg-slate-50"
          >
            <h3 className="font-semibold">{ticket.title}</h3>
            <p className="text-sm text-gray-600">{ticket.description}</p>
            <p className="text-sm mt-2">Status: {ticket.status}</p>
            <p className="text-sm text-gray-500">
              Assigned To: {ticket.assignedToUserEmail || "Unassigned"}
            </p>
          </Link>
        ))}
      </div>
    </div>
  );
}