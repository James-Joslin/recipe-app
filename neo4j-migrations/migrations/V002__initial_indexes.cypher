CREATE INDEX recipe_title_index IF NOT EXISTS
FOR (recipe:Recipe) ON (recipe.title)

