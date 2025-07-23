'use client';

import Link from 'next/link';
import { BookMarked } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { ThemeToggle } from '@/components/ThemeToggle';

export default function Navbar() {
  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/80 backdrop-blur-sm px-4">
      <div className="container flex h-16 items-center justify-between mx-auto max-w-7xl">
        <div className="flex items-center gap-2">
          <BookMarked className="w-6 h-6" />
          <Link href="/" className="text-xl font-bold tracking-tight">
            Blotz Dev Share
          </Link>
        </div>

        <nav className="hidden md:flex items-center gap-8">
          <Link
            href="/"
            className="text-md font-medium text-muted-foreground transition-colors hover:text-foreground"
          >
            Discover
          </Link>
          <Link
            href="/share"
            className="text-md font-medium text-muted-foreground transition-colors hover:text-foreground"
          >
            Share
          </Link>
          <Link
            href="/collections"
            className="text-md font-medium text-muted-foreground transition-colors hover:text-foreground"
          >
            Collections
          </Link>
        </nav>

        <div className="flex items-center gap-4">
          <ThemeToggle />
          <div className="hidden md:block">
            <Button>Sign In</Button>
          </div>
          <DropdownMenu>
            <DropdownMenuTrigger asChild className="md:hidden">
              <Button variant="ghost" size="icon">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  className="h-5 w-5"
                >
                  <line x1="4" x2="20" y1="12" y2="12" />
                  <line x1="4" x2="20" y1="6" y2="6" />
                  <line x1="4" x2="20" y1="18" y2="18" />
                </svg>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem asChild>
                <Link href="/">Discover</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link href="/share">Share</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link href="/collections">Collections</Link>
              </DropdownMenuItem>
              <DropdownMenuItem asChild>
                <Link href="/sign-in">Sign In</Link>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    </header>
  );
}
