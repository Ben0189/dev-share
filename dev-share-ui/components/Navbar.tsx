'use client';

import Link from 'next/link';
import { BookMarked } from 'lucide-react';
import { ThemeToggle } from '@/components/ThemeToggle';

export default function Navbar() {
  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/80 backdrop-blur-sm">
      <div className="flex h-16 items-center justify-between px-12">
        <div className="flex items-center gap-2">
          <BookMarked className="w-6 h-6" />
          <Link href="/" className="text-xl font-bold tracking-tight">
            Blotz Dev Share
          </Link>
        </div>

        <nav className="hidden md:flex items-center gap-8">
          <Link
            href="/share"
            className="text-md font-medium text-muted-foreground transition-colors hover:text-foreground"
          >
            Share
          </Link>
          <ThemeToggle />
        </nav>
      </div>
    </header>
  );
}
