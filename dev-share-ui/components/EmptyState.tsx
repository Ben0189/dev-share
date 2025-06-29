"use client";

import { useState } from "react";
import { Search, Sparkles } from "lucide-react";
import { Button } from "@/components/ui/button";

interface EmptyStateProps {
  title?: string;
  description?: string;
  searchQuery?: string;
  onSearchSuggestion?: (query: string) => void;
  onGlobalSearch?: (query?: string) => void;
  showToggle?: boolean;
}

export default function EmptyState({
  title = "No resources found",
  description = "Try adjusting your search or browse our trending resources",
  searchQuery,
  onSearchSuggestion,
  onGlobalSearch,
  showToggle = false
}: EmptyStateProps) {
  const [showDevToggle, setShowDevToggle] = useState(false);

  // Common developer search terms that might yield results
  const suggestedSearches = ["React", "TypeScript", "Next.js", "JavaScript", "CSS", "Node.js"];
  
  return (
    <div className="relative flex flex-col items-center justify-center min-h-[60vh] w-full">
      {/* Floating dev toggle switch (bottom right, semi-transparent) */}
      <label
        className="fixed bottom-6 right-6 z-50 bg-black/60 text-white px-3 py-2 rounded-full shadow-lg opacity-60 hover:opacity-100 transition-opacity text-xs flex items-center gap-2 cursor-pointer"
        style={{ display: showToggle ? 'flex' : 'none' }}
        tabIndex={0}
      >
        <input
          type="checkbox"
          checked={showDevToggle}
          onChange={() => setShowDevToggle((v) => !v)}
          className="accent-indigo-500 mr-1"
        />
        Dev Toggle
      </label>

      {/* Playful SVG illustration */}
      <div className="mb-6">
        <svg width="80" height="80" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
          <circle cx="40" cy="40" r="38" fill="#F3F4F6" stroke="#E5E7EB" strokeWidth="2" />
          <ellipse cx="40" cy="38" rx="18" ry="14" fill="#fff" stroke="#D1D5DB" strokeWidth="2" />
          <circle cx="40" cy="38" r="7" fill="#A5B4FC" stroke="#6366F1" strokeWidth="2" />
          <rect x="54" y="52" width="12" height="4" rx="2" fill="#6366F1" />
          <rect x="60" y="56" width="4" height="12" rx="2" fill="#6366F1" />
          <circle cx="40" cy="38" r="3" fill="#fff" />
        </svg>
      </div>
      <h2 className="text-xl font-semibold mb-2">Nothing here yet!</h2>
      <p className="text-gray-500 mb-4 text-center max-w-md">
        {searchQuery
          ? <>We couldn&apos;t find anything related to <span className="font-semibold text-gray-700">&quot;{searchQuery}&quot;</span>.</>
          : <>We couldn&apos;t find any resources matching your search.<br/>Try different keywords, or let SuperPro AI help you discover something new.</>}
      </p>
      {/* CTA card */}
      <div className="bg-gray-100 border border-gray-300 rounded-xl p-6 flex flex-col items-center w-full max-w-md shadow-md mb-2">
        <div className="flex items-center gap-2 mb-2">
          <Sparkles className="w-5 h-5 text-indigo-500" />
          <span className="font-semibold text-gray-800">SuperPro AI Global Search</span>
        </div>
        <p className="text-gray-500 text-center mb-4 text-sm">Can&apos;t find what you are looking for? Let our AI search the web for you!</p>
        <Button
          className="bg-black text-white font-semibold px-6 py-2 rounded-lg shadow hover:bg-indigo-700 flex items-center gap-2 text-base transition-colors"
          onClick={() => onGlobalSearch && onGlobalSearch()}
        >
          <Sparkles className="w-5 h-5" />
          Let SuperPro AI do a global search
        </Button>
      </div>
    </div>
  );
}