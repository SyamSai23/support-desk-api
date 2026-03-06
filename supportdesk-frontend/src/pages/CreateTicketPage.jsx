import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";

export default function CreateTicketPage() {
  const [form, setForm] = useState({ title: "", description: "" });
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const res = await api.post("/tickets", form);
      navigate(`/tickets/${res.data.id}`);
    } catch (err) {
      setError("Failed to create ticket");
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-10 bg-white p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Create Ticket</h2>

      {error && <p className="text-red-500 mb-3">{error}</p>}

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input
          name="title"
          placeholder="Title"
          className="border p-2 rounded"
          onChange={handleChange}
        />
        <textarea
          name="description"
          placeholder="Description"
          className="border p-2 rounded"
          rows="5"
          onChange={handleChange}
        />
        <button className="bg-blue-600 text-white py-2 rounded">Create</button>
      </form>
    </div>
  );
}