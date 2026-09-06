import { NextResponse } from 'next/server';

export async function GET() {
  try {
    const response = await fetch(`${process.env.RECIPE_API_URL || 'http://api:8080'}/api/recipes`, {
      cache: 'no-store',
    });
    const body = await response.json();
    return NextResponse.json(body, { status: response.status });
  } catch {
    return NextResponse.json({ error: 'The recipe API is unavailable.' }, { status: 503 });
  }
}

