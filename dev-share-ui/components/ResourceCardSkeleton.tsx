import { Skeleton } from '@/components/ui/skeleton';
import { Card } from './ui/card';

export default function ResourceCardSkeleton() {
  return (
    <Card className="flex flex-col justify-between min-h-[320px] min-w-0 p-0 rounded-xl border shadow-sm bg-white transition-all duration-300 hover:shadow-md relative animate-pulse">
      {/* Top - Avatar and Meta */}
      <div className="flex items-center gap-3 px-6 pt-6 pb-2">
        <Skeleton className="h-8 w-8 rounded-full" />
        <div className="flex flex-col gap-2 flex-1">
          <Skeleton className="h-3 w-3/4" />
          <Skeleton className="h-2 w-1/2" />
        </div>
      </div>

      {/* Comment line (tooltip in real) */}
      <div className="px-6 pb-1">
        <Skeleton className="h-3 w-4/5" />
      </div>

      {/* Title, Description, Tags */}
      <div className="flex-1 px-6 pt-2 pb-0 flex flex-col gap-2">
        <Skeleton className="h-4 w-4/5" />
        <Skeleton className="h-3 w-full" />
        <Skeleton className="h-3 w-5/6" />

        {/* Tags */}
        <div className="flex flex-wrap gap-2 mt-auto mb-2">
          <Skeleton className="h-5 w-12 rounded-full" />
          <Skeleton className="h-5 w-16 rounded-full" />
          <Skeleton className="h-5 w-10 rounded-full" />
        </div>
      </div>

      {/* Footer */}
      <div className="flex items-center justify-between px-6 py-4 border-t mt-2">
        <div className="flex gap-6">
          <Skeleton className="h-4 w-12" />
          <Skeleton className="h-4 w-16" />
        </div>
        <Skeleton className="h-8 w-20 rounded" />
      </div>
    </Card>
  );
}
