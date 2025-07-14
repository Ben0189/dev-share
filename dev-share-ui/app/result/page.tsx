'use client';

import { useEffect, useState } from 'react';
import { Resource } from '@/lib/types';
import ResourceCard from '@/components/ResourceCard';
import { useParams, usePathname } from 'next/navigation';
import { handleGlobalSearch, searchResources } from '@/services/search-service';
import { mockResources } from '@/lib/data';

export default function SearchPage(term: string) {
  const [resources, setResources] = useState<Resource[]>(mockResources);
  const [loading, setLoading] = useState(true);
  const { term: paramTerm } = useParams<{ term: string }>();
  const path = usePathname();
  const searchTerm = path.replace('/result/', '');
  //   useEffect(() => {
  //     // in real life hit a DB or external API.
  //     // here we just call our own route handler below.
  //     (async () => {
  //       const result = await searchResources(searchTerm);
  //       if (result.length === 0) {
  //         await handleGlobalSearch();
  //       } else {
  //         setResources(result);
  //       }
  //       setResources(result);
  //       setLoading(false);
  //     })();
  //   }, [searchTerm]);
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

  return (
    <main className="min-h-screen bg-background">
      <div className="container px-4 py-8 mx-auto max-w-7xl">
        <div className="flex items-center justify-between mb-8">
          <div className="flex items-center gap-4">
            <span className="text-lg font-medium text-muted-foreground">
              {`${resources.length} resources found`}
            </span>
          </div>
          <a
            href="#"
            className="text-primary hover:underline flex items-center gap-1 text-sm font-medium"
          >
            View All <span aria-hidden="true">→</span>
          </a>
        </div>
        {
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {resources
              .sort((a, b) => b.likes - a.likes)
              .map((resource, idx) => (
                <ResourceCard
                  key={resource.id}
                  resource={resource}
                  onAction={handleResourceAction}
                />
              ))}
          </div>
        }
      </div>
    </main>
  );
}
