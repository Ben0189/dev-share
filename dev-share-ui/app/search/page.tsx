"use client";

import { useState } from "react";
import Link from "next/link";
import Navbar from "@/components/Navbar";
import HeroSection from "@/components/HeroSection";
import ResourceGrid from "@/components/ResourceGrid";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { mockResources } from "@/lib/data";
import { Resource, VectorSearchResultDTO } from "@/lib/types";
import EmptyState from "@/components/EmptyState";
import { Switch } from "@/components/ui/switch";

export default function SearchPage() {
  const [resources, setResources] = useState(mockResources);
  const [isSearching, setIsSearching] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [showEmpty, setShowEmpty] = useState(false);
  const topRelative = 6;

  const searchResources = async(query: string) : Promise<Resource[]>=> {
    const result = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL_WITH_API}/search`,{
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        text: query,
        topRelatives: topRelative
      }),
    })

    if (!result.ok) throw new Error(`Search failed (${result.status})`);

    const dtos: VectorSearchResultDTO[] = await result.json();

    return dtos.map(dto => ({
        id: crypto.randomUUID(),            // or hash(dto.url)
        title: dto.content.slice(0, 80),    // quick placeholder
        description: dto.content,
        url: dto.url,
        imageUrl: "",                       // TODO: fetch or derive
        tags: [],
        likes: 0,
        date: new Date().toISOString(),
        isLiked: false,
        isBookmarked: false,
        recommended: false,
        authorName: "Unknown",
        authorAvatar: "https://avatar.iran.liara.run/public/boy",
        linkClicks: 0,
        createdAt: new Date().toISOString()
    }));
  };

  const handleSearch = async (query: string) => {
    setIsSearching(true);
    setSearchQuery(query);
    
    const result = await searchResources(query);

    setResources(result);
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
            <span className="text-lg font-medium text-muted-foreground">{resources.length} resources found</span>
            <label className="flex items-center gap-2 text-xs text-muted-foreground cursor-pointer select-none">
              <Switch checked={showEmpty} onCheckedChange={() => setShowEmpty((v) => !v)} />
              Dev Toggle
            </label>
          </div>
          <a href="#" className="text-primary hover:underline flex items-center gap-1 text-sm font-medium">View All <span aria-hidden="true">→</span></a>
        </div>
        {showEmpty ? (
          <EmptyState
            searchQuery={searchQuery}
            onGlobalSearch={(q) => alert(`SuperPro AI global search for: ${q || ''}`)}
            showToggle={true}
          />
        ) : (
          <ResourceGrid 
            resources={resources.sort((a, b) => b.likes - a.likes)} 
            onAction={handleResourceAction} 
            isLoading={isSearching}
            searchQuery={searchQuery}
            onSearchSuggestion={handleSearchSuggestion}
          />
        )}
      </div>
    </main>
  );
}