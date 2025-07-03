"use client";

import { useState } from "react";
import Navbar from "@/components/Navbar";
import HeroSection from "@/components/HeroSection";
import { mockResources } from "@/lib/data";
import { Resource, VectorSearchResultDTO } from "@/lib/types";
import ResourceCard from "@/components/ResourceCard";
import FromAISearchResourceCard from "@/components/FromAISearchResourceCard";
import{handleGlobalSearch,searchResources} from "@/services/search-service";

export default function SearchPage() {
  const [resources, setResources] = useState(mockResources);
  const [isSearching, setIsSearching] = useState(false);
  const [SearchQuery, setSearchQuery] = useState("");
  const [isAIFallback, setIsAIFallback] = useState(false);
  const topRelative = 6;



  const handleSearch = async (query: string) => {
    setIsSearching(true);
    setSearchQuery(query);
    setResources([]);
    
    const result = await searchResources(query);
    if (result.length === 0) {
      await handleGlobalSearch();
      setIsAIFallback(true);
    } else {
      setResources(result);
    }
    setIsSearching(false);
  };

  const handleResourceAction = (id: string, action: 'like' | 'bookmark') => {
    // TODO: Integrate with feedback loop API
    // Integration point for feedback loop:
    // 1. Send user action to your API
    // 2. Update resource in your database
    // 3. Use this data to improve recommendations
    
    setResources(prevResources => 
      prevResources.map(resource => {
        if (resource.id === id) {
          if (action === 'like') {
            return { ...resource, likes: resource.isLiked ? resource.likes - 1 : resource.likes + 1, isLiked: !resource.isLiked };
          } else {
            return { ...resource, isBookmarked: !resource.isBookmarked };
          }
        }
        return resource;
      })
    );
  };
  
  const handleSearchSuggestion = (suggestion: string) => {
    handleSearch(suggestion);
  };


  return (
    <main className="min-h-screen bg-background">
      <Navbar />
      <HeroSection onSearch={handleSearch} isSearching={isSearching} />
      
      <div className="container px-4 py-8 mx-auto max-w-7xl">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-4">
          <span className="text-lg font-medium text-muted-foreground">
            {isAIFallback ? "No direct results found. Showing AI suggestions instead." : `${resources.length} resources found`}</span>
          </div>
          <a href="#" className="text-primary hover:underline flex items-center gap-1 text-sm font-medium">View All <span aria-hidden="true">→</span></a>
        </div>
        { (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {resources.sort((a, b) => b.likes - a.likes).map((resource, idx) =>
              resource.isAIGenerated ? (
                <FromAISearchResourceCard key={resource.id} resource={resource} onAction={handleResourceAction} />
              ) : (
                <ResourceCard key={resource.id} resource={resource} onAction={handleResourceAction} />
              )
            )}
          </div>
        )}
      </div>
    </main>
  );
}