'use client';

import { useState, useEffect } from 'react';
import { Search } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { useSearchParams, usePathname, useRouter } from 'next/navigation';
import { useDebouncedCallback } from 'use-debounce';

export default function SearchBar() {
  const router = useRouter();
  const [query, setQuery] = useState('');
  const [placeholderIndex, setPlaceholderIndex] = useState(0);
  const searchParams = useSearchParams();
  const pathname = usePathname();
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

  const handleSearch = useDebouncedCallback((term) => {
    const params = new URLSearchParams(Array.from(searchParams.entries()));
    if (term) {
      params.set('query', term);
    } else {
      params.delete('query');
    }

    router.replace(`${pathname}?${params.toString()}`);
  }, 400);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (query.trim()) {
      router.push(`/result/?query=${query}`);
    }
  };
  return (
    <form
      onSubmit={handleSubmit}
      className="relative w-full flex-1 flex items-center justify-center"
    >
      <Search className="relative left-8 text-muted-foreground" />
      <Input
        type="search"
        placeholder={placeholders[placeholderIndex]}
        onChange={(e) => {
          handleSearch(e.target.value);
          setQuery(e.target.value);
        }}
        defaultValue={searchParams.get('query')?.toString()}
        className="pl-10 h-11 rounded-lg bg-card transition-all duration-300 focus:ring-2 focus:ring-primary/20 w-3/5"
      />
    </form>
  );
}
