'use client';

import useSWR from 'swr';
import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { useDebounce } from 'use-debounce';
import { searchResources } from '@/services/search-service';
import ResourceCard from './ResourceCard';
import ResourceTableSkeleton from './ResourceTableSkeleton';
import { Resource } from '@/lib/types';

const fetcher = (query: string) => searchResources(query);

export default function ResourceTable() {
  const searchParams = useSearchParams();
  const query = searchParams.get('query') || '';
  const [debouncedQuery] = useDebounce(query, 400);

  const { data, isLoading } = useSWR(
    debouncedQuery.trim() ? ['resources', debouncedQuery] : null,
    () => fetcher(debouncedQuery.trim()),
    {
      revalidateOnFocus: false,
      dedupingInterval: 5000,
    }
  );

  const [resources, setResources] = useState<Resource[]>([]);

  // Sync local state with fetched data
  useEffect(() => {
    if (data) {
      setResources(data);
    }
  }, [data]);

  const handleResourceAction = (id: string, action: 'like' | 'bookmark') => {
    setResources((prevResources) =>
      prevResources.map((resource) => {
        if (resource.id !== id) return resource;

        if (action === 'like') {
          return {
            ...resource,
            likes: resource.isLiked ? resource.likes - 1 : resource.likes + 1,
            isLiked: !resource.isLiked,
          };
        }

        return { ...resource, isBookmarked: !resource.isBookmarked };
      })
    );

    // Optional: send update to API
  };

  if (isLoading) {
    return <ResourceTableSkeleton />;
  }

  return (
    <div className="container px-4 py-8 mx-auto max-w-7xl">
      <div className="mb-8">
        <span className="text-lg text-muted-foreground">
          {`${resources.length} resources found`}
        </span>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {resources.map((r) => (
          <ResourceCard
            key={r.id}
            resource={r}
            onAction={handleResourceAction}
          />
        ))}
      </div>
    </div>
  );
}
