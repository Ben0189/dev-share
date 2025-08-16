'use client';

import { useState, useEffect } from 'react';
import { Particles } from '@/components/magicui/particles';
import { useTheme } from 'next-themes';
import SearchBar from './SearchBar';

export default function HeroSection() {
  const { resolvedTheme } = useTheme();
  const [color, setColor] = useState('#ffffff');

  useEffect(() => {
    setColor(resolvedTheme === 'dark' ? '#ffffff' : '#000000');
  }, [resolvedTheme]);

  return (
    <section className="w-full bg-background">
      <Particles
        className="absolute inset-0 z-0"
        ease={80}
        color={color}
        refresh
      />
      <div className="container px-4 mx-auto max-w-7xl">
        <div className="flex flex-col gap-8 py-10">
          <div className="flex flex-row items-center justify-center w-full mb-2">
            <div className="flex flex-col gap-6 ">
              <h2 className="text-5xl font-bold tracking-tight text-center text-foreground mb-1">
                Discover Resources
              </h2>
              <p className="text-sm text-muted-foreground text-center">
                Find and share valuable community resources
              </p>
            </div>
          </div>
          <SearchBar />
        </div>
      </div>
    </section>
  );
}
