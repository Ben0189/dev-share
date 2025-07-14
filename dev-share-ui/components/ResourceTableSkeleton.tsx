import ResourceCardSkeleton from './ResourceCardSkeleton';

export default function ResourceTableSkeleton() {
  return (
    <div className="container px-4 py-8 mx-auto max-w-7xl min-h-[800px]">
      <div className="flex items-center justify-between mb-8">
        <span className="text-lg font-medium text-muted-foreground">
          Loading resources...
        </span>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {Array(6)
          .fill(1)
          .map((index) => (
            <ResourceCardSkeleton key={index} />
          ))}
      </div>
    </div>
  );
}
