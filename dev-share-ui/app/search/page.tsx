'use client';

import { useState } from 'react';
import HeroSection from '@/components/HeroSection';
import { mockResources } from '@/lib/data';
import ResourceCard from '@/components/ResourceCard';
import FromAISearchResourceCard from '@/components/FromAISearchResourceCard';
import { handleGlobalSearch, searchResources } from '@/services/search-service';

export default function SearchPage() {
  const [resources, setResources] = useState(mockResources);
  const [isSearching, setIsSearching] = useState(false);
  const [SearchQuery, setSearchQuery] = useState('');
  const topRelative = 6;

  const handleSearch = async (query: string) => {
    setIsSearching(true);
    setSearchQuery(query);
    setResources([]);

    const result = await searchResources(query);
    if (result.length === 0) {
      await handleGlobalSearch();
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

    setResources((prevResources) =>
      prevResources.map((resource) => {
        if (resource.id === id) {
          if (action === 'like') {
            return {
              ...resource,
              likes: resource.isLiked ? resource.likes - 1 : resource.likes + 1,
              isLiked: !resource.isLiked,
            };
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
    <div className="bg-background flex grow pb-16 justify-center items-center">
      <HeroSection onSearch={handleSearch} isSearching={isSearching} />
    </div>
  );
}
