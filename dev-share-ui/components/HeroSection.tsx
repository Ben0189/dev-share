'use client';

import { useState, useEffect } from 'react';
import { Search, Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Particles } from '@/components/magicui/particles';
import { useTheme } from 'next-themes';

interface HeroSectionProps {
  onSearch: (query: string) => void;
  isSearching: boolean;
}

export default function HeroSection({
  onSearch,
  isSearching,
}: HeroSectionProps) {
  const [query, setQuery] = useState('');
  const [placeholderIndex, setPlaceholderIndex] = useState(0);
  const { resolvedTheme } = useTheme();
  const [color, setColor] = useState('#ffffff');

  useEffect(() => {
    setColor(resolvedTheme === 'dark' ? '#ffffff' : '#000000');
  }, [resolvedTheme]);
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
    if (query.trim()) {
      onSearch(query);
    }
  };

  return (
    <section className="w-full py-10 md:py-10 bg-background border-b">
      <Particles
        className="absolute inset-0 z-0"
        ease={80}
        color={color}
        refresh
      />
      <div className="container px-4 py-11 mx-auto max-w-7xl">
        <div className="flex flex-col gap-8 py-10">
          <div className="flex flex-row items-center justify-center w-full mb-2">
            <div className="flex flex-col gap-6 ">
              <h2 className="text-5xl font-bold tracking-tight text-foreground mb-1">
                Discover Resources
              </h2>
              <p className="text-sm text-muted-foreground text-center">
                Find and share valuable community resources
              </p>
            </div>
          </div>
          <form onSubmit={handleSubmit} className="flex w-full space-x-2">
            <div className="relative flex-1 flex items-center justify-center">
              <Search className="relative left-8 text-muted-foreground" />
              <Input
                type="text"
                placeholder={placeholders[placeholderIndex]}
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="pl-10 h-11 rounded-lg bg-card transition-all duration-300 focus:ring-2 focus:ring-primary/20 w-3/5"
                disabled={isSearching}
              />
            </div>
          </form>
        </div>
      </div>
    </section>
  );
}
