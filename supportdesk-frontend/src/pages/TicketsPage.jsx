import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../api/axios";

export default function TicketsPage() {
  const [tickets, setTickets] = useState([]);
  const [error, setError] = useState("");
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");

  const fetchTickets = async () => {
    try {
      const params = new URLSearchParams();

      if (search.trim()) params.append("search", search.trim());
      if (status) params.append("status", status);

      const res = await api.get(`/tickets?${params.toString()}`);
      setTickets(res.data.items || []);
    } catch {
      setError("Failed to load tickets");
    }
  };

  useEffect(() => {
    fetchTickets();
  }, []);

  const handleFilter = (e) => {
    e.preventDefault();
    fetchTickets();
  };

  return (
    <div className="max-w-5xl mx-auto mt-8 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Tickets</h2>

      <form onSubmit={handleFilter} className="flex gap-3 mb-6">
        <input
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search tickets..."
          className="border p-2 rounded flex-1"
        />

        <select
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          className="border p-2 rounded"
        >
          <option value="">All Statuses</option>
          <option value="Open">Open</option>
          <option value="InProgress">InProgress</option>
          <option value="Closed">Closed</option>
        </select>

        <button className="bg-blue-600 text-white px-4 py-2 rounded">
          Apply
        </button>
      </form>

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
          </Link>
        ))}
      </div>
    </div>
  );
}