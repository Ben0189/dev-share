'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { useDebounce } from 'use-debounce';
import { searchResources } from '@/services/search-service';
import ResourceCard from './ResourceCard';
import ResourceTableSkeleton from './ResourceTableSkeleton';

export default function ResourceTable() {
  const searchParams = useSearchParams();
  const query = searchParams.get('query') || '';
  const [debouncedQuery] = useDebounce(query, 400);
  const [resources, setResources] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

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
          <ResourceCard key={r.id} resource={r} onAction={() => {}} />
        ))}
      </div>
    </div>
  );
}
