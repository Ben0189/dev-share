"use client";

import { Resource } from "@/lib/types";
import ResourceCard from "@/components/ResourceCard";
import EmptyState from "@/components/EmptyState";
import { Skeleton } from "@/components/ui/skeleton";

interface ResourceGridProps {
  resources: Resource[];
  onAction: (id: string, action: 'like' | 'bookmark') => void;
  isLoading?: boolean;
  searchQuery?: string;
  onSearchSuggestion?: (query: string) => void;
}

export default function ResourceGrid({ 
  resources, 
  onAction, 
  isLoading = false, 
  searchQuery,
  onSearchSuggestion
}: ResourceGridProps) {
  // Create skeleton array for loading state
  const skeletonCards = Array.from({ length: 6 }, (_, i) => i);

  const handleResourceAnimation = (index: number) => {
    // Add staggered animation delay based on item index
    return {
      animationDelay: `${index * 0.05}s`
    };
  };

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {skeletonCards.map((i) => (
          <div key={i} className="space-y-3">
            <Skeleton className="h-40 w-full rounded-md" />
            <div className="space-y-2">
              <Skeleton className="h-5 w-4/5" />
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-3/4" />
            </div>
            <div className="flex justify-between items-center pt-2">
              <div className="flex space-x-2">
                <Skeleton className="h-8 w-16 rounded-md" />
                <Skeleton className="h-8 w-8 rounded-md" />
              </div>
              <Skeleton className="h-8 w-16 rounded-md" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (resources.length === 0) {
    return (
      <EmptyState 
        searchQuery={searchQuery} 
        onSearchSuggestion={onSearchSuggestion}
      />
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {resources.map((resource, index) => (
        <div 
          key={resource.id} 
          className="animate-in fade-in slide-in-from-bottom-4 duration-500 fill-mode-both"
          style={handleResourceAnimation(index)}
        >
          <ResourceCard 
            resource={resource} 
            onAction={onAction} 
            isAIGenerated={resource.isAIGenerated}
          />
        </div>
      ))}
    </div>
  );
}