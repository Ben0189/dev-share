import SearchBar from '@/components/SearchBar';
import ResourceTable from '@/components/ResourceTable';

export default function ResultPage() {
  return (
    <div className="min-h-screen bg-background py-10 flex flex-col gap-4 items-center px-20">
      <div className="w-full max-w-7xl">
        <SearchBar />
      </div>
      <ResourceTable />
    </div>
  );
}
