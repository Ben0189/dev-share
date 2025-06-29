"use client";

import { useState, useEffect } from "react";
import { Search, Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Link from "next/link";

interface HeroSectionProps {
  onSearch: (query: string) => void;
  isSearching: boolean;
}

export default function HeroSection({ onSearch, isSearching }: HeroSectionProps) {
  const [query, setQuery] = useState("");
  const [placeholderIndex, setPlaceholderIndex] = useState(0);
  
  const placeholders = [
    "Search resources, tools, guides...",
    "TypeScript tutorials for beginners",
    "How to optimize Next.js performance",
    "GraphQL resources for frontend developers",
    "Learn CSS Grid and Flexbox",
    "Node.js backend architecture patterns"
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
    <section className="w-full py-8 md:py-10 bg-background border-b">
      <div className="container px-4 py-8 mx-auto max-w-7xl">
        <div className="flex flex-col gap-6">
          <div className="flex flex-row items-center justify-between w-full mb-2">
            <div>
              <h2 className="text-2xl font-bold tracking-tight text-foreground mb-1 text-left">Discover Resources</h2>
              <p className="text-sm text-muted-foreground text-left">Find and share valuable community resources</p>
            </div>
            <Button asChild className="h-11 px-5 bg-primary text-white hover:bg-primary/90">
              <Link href="/share">
                <Plus className="h-4 w-4 mr-2" />
                Share Resource
              </Link>
            </Button>
          </div>
          <form onSubmit={handleSubmit} className="flex w-full items-center space-x-2">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder={placeholders[placeholderIndex]}
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="pl-10 h-11 rounded-lg bg-card transition-all duration-300 focus:ring-2 focus:ring-primary/20 w-full"
                disabled={isSearching}
              />
            </div>
          </form>
        </div>
      </div>
    </section>
  );
}