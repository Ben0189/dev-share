'use client';

import { handleGlobalSearch, searchResources } from '@/services/search-service';
import ResourceCard from './ResourceCard';
import { mockResources } from '@/lib/data';

interface ResourceTableProps {
  query: string;
}

export default async function ResourceTable({ query }: ResourceTableProps) {
  const resources = await searchResources(query);

  const handleResourceAction = (id: string, action: 'like' | 'bookmark') => {
    // TODO: Integrate with feedback loop API
    // Integration point for feedback loop:
    // 1. Send user action to your API
    // 2. Update resource in your database
    // 3. Use this data to improve recommendations
    // setResources((prevResources) =>
    //   prevResources.map((resource) => {
    //     if (resource.id === id) {
    //       if (action === 'like') {
    //         return {
    //           ...resource,
    //           likes: resource.isLiked ? resource.likes - 1 : resource.likes + 1,
    //           isLiked: !resource.isLiked,
    //         };
    //       } else {
    //         return { ...resource, isBookmarked: !resource.isBookmarked };
    //       }
    //     }
    //     return resource;
    //   })
    // );
  };
  return (
    <div className="container px-4 py-8 mx-auto max-w-7xl">
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-4">
          <span className="text-lg font-medium text-muted-foreground">
            {`${resources.length} resources found`}
          </span>
        </div>
      </div>
      {
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {resources
            .sort((a, b) => b.likes - a.likes)
            .map((resource) => (
              <ResourceCard
                key={resource.id}
                resource={resource}
                onAction={handleResourceAction}
              />
            ))}
        </div>
      }
    </div>
  );
}
