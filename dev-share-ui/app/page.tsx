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

export default function Home() {
  const [resources, setResources] = useState(mockResources);
  const [isSearching, setIsSearching] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const topRelative = 6;

  const searchResources = async(query: string) : Promise<Resource[]>=> {
    const result = await fetch('https://localhost:7122/api/search',{
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
        recommended: false
    }));
  };

  const handleSearch = async (query: string) => {
    setIsSearching(true);
    setSearchQuery(query);
    
    const result = await searchResources(query);
    
    // TODO: Integrate with Azure OpenAI for embeddings and search
    // Integration point for Azure OpenAI API:
    // 1. Create embeddings for the search query
    // 2. Search for similar embeddings in your vector database
    // 3. Return the most relevant resources
    // Example:
    // const embeddings = await createEmbeddings(query);
    // const results = await searchSimilarResources(embeddings);
    // setResources(results);
    
    const filtered = mockResources.filter(resource => 
      resource.title.toLowerCase().includes(query.toLowerCase()) || 
      resource.description.toLowerCase().includes(query.toLowerCase()) ||
      resource.tags.some(tag => tag.toLowerCase().includes(query.toLowerCase()))
    );
    
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
        {/* Share Resource Button */}
        <div className="mb-8 flex justify-center">
          <Button 
            asChild
            className="group relative overflow-hidden transition-all duration-300"
            size="lg"
          >
            <Link href="/share">
              <span className="flex items-center gap-2">
                <Plus className="h-4 w-4 transition-transform group-hover:rotate-90 duration-300" />
                Share a Resource
              </span>
            </Link>
          </Button>
        </div>

        <Tabs defaultValue="trending" className="w-full">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-8 gap-4">
            <h2 className="text-2xl font-semibold tracking-tight">Discover Resources</h2>
            <TabsList className="h-10">
              <TabsTrigger value="trending" className="px-4">Trending</TabsTrigger>
              <TabsTrigger value="new" className="px-4">New</TabsTrigger>
              <TabsTrigger value="for-you" className="px-4">For You</TabsTrigger>
            </TabsList>
          </div>
          
          <TabsContent value="trending" className="mt-0">
            <ResourceGrid 
              resources={resources.sort((a, b) => b.likes - a.likes)} 
              onAction={handleResourceAction} 
              isLoading={isSearching}
              searchQuery={searchQuery}
              onSearchSuggestion={handleSearchSuggestion}
            />
          </TabsContent>
          
          <TabsContent value="new" className="mt-0">
            <ResourceGrid 
              resources={resources.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())} 
              onAction={handleResourceAction}
              isLoading={isSearching}
              searchQuery={searchQuery}
              onSearchSuggestion={handleSearchSuggestion}
            />
          </TabsContent>
          
          <TabsContent value="for-you" className="mt-0">
            <ResourceGrid 
              resources={resources.filter(r => r.recommended)} 
              onAction={handleResourceAction}
              isLoading={isSearching}
              searchQuery={searchQuery}
              onSearchSuggestion={handleSearchSuggestion}
            />
          </TabsContent>
        </Tabs>
      </div>
    </main>
  );
}