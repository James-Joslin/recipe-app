import './globals.css';

export const metadata = {
  title: 'Recipe App',
  description: 'A small full-stack recipe workspace.',
};

export default function RootLayout({ children }) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}

