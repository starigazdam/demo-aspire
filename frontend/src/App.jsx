import { useEffect, useState } from 'react'
import './App.css'

function App() {
  const [todos, setTodos] = useState([])
  const [title, setTitle] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    fetch('/api/todos')
      .then((response) => response.ok ? response.json() : Promise.reject())
      .then(setTodos)
      .catch(() => setError('The API is not available yet. Start the AppHost.'))
  }, [])

  async function addTodo(event) {
    event.preventDefault()
    const response = await fetch('/api/todos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title }),
    })

    if (!response.ok) return setError('Could not save that item.')

    setTodos([...todos, await response.json()])
    setTitle('')
    setError('')
  }

  return <main>
    <p className="eyebrow">.NET Aspire demo</p>
    <h1>Small app, real wiring.</h1>
    <p>React talks to an Azure Function API; Aspire runs both with SQL Server locally.</p>
    <form onSubmit={addTodo}>
      <label htmlFor="title">Add a talking point</label>
      <div className="row">
        <input id="title" value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Show the Aspire dashboard" required />
        <button>Add</button>
      </div>
    </form>
    {error && <p className="error" role="alert">{error}</p>}
    <ul>{todos.map((todo) => <li key={todo.id}>{todo.title}</li>)}</ul>
  </main>
}

export default App
