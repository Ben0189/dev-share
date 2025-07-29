'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { useDebounce } from 'use-debounce';
import { searchResources } from '@/services/search-service';
import ResourceCard from './ResourceCard';
import ResourceTableSkeleton from './ResourceTableSkeleton';
import { Resource } from '@/lib/types';

export default function ResourceTable() {
  const searchParams = useSearchParams();
  const query = searchParams.get('query') || '';
  const [debouncedQuery] = useDebounce(query, 400);
  const [resources, setResources] = useState<Resource[]>([]);
  const [loading, setLoading] = useState(true);
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

  useEffect(() => {
    let cancelled = false;

    const fetchResources = async () => {
      setLoading(true);
      const result = debouncedQuery.trim()
        ? await searchResources(debouncedQuery)
        : [];
      if (!cancelled) {
        setResources(result);
        setLoading(false);
      }
    };

    fetchResources();
    return () => {
      cancelled = true;
    };
  }, [debouncedQuery]);

  if (loading) return <ResourceTableSkeleton />;

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
