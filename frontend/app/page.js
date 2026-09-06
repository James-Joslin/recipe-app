'use client';

import { useEffect, useState } from 'react';

const pythonUrl = process.env.NEXT_PUBLIC_FASTAPI_URL || 'http://localhost:8001';

export default function HomePage() {
  const [recipes, setRecipes] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch('/api/recipes')
      .then((response) => {
        if (!response.ok) throw new Error('The API is unavailable.');
        return response.json();
      })
      .then(setRecipes)
      .catch((requestError) => setError(requestError.message));
  }, []);

  return (
    <main className="shell">
      <header className="hero">
        <p className="eyebrow">Recipe App</p>
        <h1>A calm place for recipes.</h1>
        <p className="lede">
          This starter UI is connected to the C# API and leaves room for richer
          recipe, ingredient, and graph workflows.
        </p>
      </header>

      <section className="card" aria-labelledby="recipes-heading">
        <div className="card-heading">
          <div>
            <p className="eyebrow">Collection</p>
            <h2 id="recipes-heading">Recipes</h2>
          </div>
          <span className="count">{recipes.length}</span>
        </div>
        {error ? <p className="message error">{error}</p> : null}
        {!error && recipes.length === 0 ? (
          <p className="message">No recipes yet. Add the first one through the API.</p>
        ) : null}
        <ul className="recipe-list">
          {recipes.map((recipe) => (
            <li key={recipe.id}>
              <strong>{recipe.title}</strong>
              {recipe.description ? <span>{recipe.description}</span> : null}
            </li>
          ))}
        </ul>
      </section>

      <footer>
        <span>Next.js + ASP.NET Core + FastAPI</span>
        <a href={`${pythonUrl}/docs`}>Python service docs</a>
      </footer>
    </main>
  );
}

