'use client';

import { useState, useRef, useEffect } from 'react';
import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { useRouter, useSearchParams } from 'next/navigation';

export default function SearchBar() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [input, setInput] = useState(searchParams.get('query') || '');
  const [placeholderIndex, setPlaceholderIndex] = useState(0);
  const placeholders = [
    'Search resources, tools, guides...',
    'TypeScript tutorials for beginners',
    'How to optimize Next.js performance',
    'GraphQL resources for frontend developers',
    'Learn CSS Grid and Flexbox',
    'Node.js backend architecture patterns',
  ];
  // Rotate placeholder text every 5 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      setPlaceholderIndex((prev) => (prev + 1) % placeholders.length);
    }, 5000);

    return () => clearInterval(interval);
  }, [placeholders.length]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (input && input !== searchParams.get('query')) {
      router.push(`/result?query=${encodeURIComponent(input)}`);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="relative w-full flex items-center justify-center"
    >
      <Search className="relative left-8 text-muted-foreground" />
      <Input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        placeholder={placeholders[placeholderIndex]}
        className="pl-10 h-11 rounded-lg bg-card w-3/5"
      />
    </form>
  );
}
