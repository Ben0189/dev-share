import SearchBar from '@/components/SearchBar';
import ResourceTable from '@/components/ResourceTable';
import { Suspense } from 'react';
import ResourceTableSkeleton from '@/components/ResourceTableSkeleton';

export default async function ResultPage(props: {
  searchParams?: Promise<{
    query?: string;
    page?: string;
  }>;
}) {
  const searchParams = await props.searchParams;
  const query = searchParams?.query || '';
  const currentPage = Number(searchParams?.page) || 1;

  return (
    <div className="min-h-screen bg-background py-10 flex flex-col gap-4 items-center px-20">
      <div className="w-full max-w-7xl">
        <SearchBar />
      </div>
      <Suspense key={query + currentPage} fallback={<ResourceTableSkeleton />}>
        <ResourceTable query={query} />
      </Suspense>
    </div>
  );
}
