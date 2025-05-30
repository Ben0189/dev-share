import { Resource } from "@/lib/types";

export const mockResources: Resource[] = [
  {
    id: "1",
    title: "React Documentation",
    description: "Official React documentation with guides, API references, and examples for beginners and experienced developers.",
    url: "https://react.dev",
    imageUrl: "https://images.pexels.com/photos/11035380/pexels-photo-11035380.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["React", "JavaScript", "Frontend"],
    likes: 245,
    date: "2023-01-15",
    isLiked: false,
    isBookmarked: false,
    recommended: true
  },
  {
    id: "2",
    title: "Next.js Learn Course",
    description: "An interactive course that teaches you how to build full-stack web applications with Next.js.",
    url: "https://nextjs.org/learn",
    imageUrl: "https://images.pexels.com/photos/4974915/pexels-photo-4974915.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["Next.js", "React", "Full-stack"],
    likes: 189,
    date: "2023-03-22",
    isLiked: false,
    isBookmarked: false,
    recommended: true
  },
  {
    id: "3",
    title: "TypeScript Handbook",
    description: "Comprehensive guide to TypeScript features with examples and best practices for type-safe code.",
    url: "https://www.typescriptlang.org/docs/handbook/",
    imageUrl: "https://images.pexels.com/photos/4709285/pexels-photo-4709285.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["TypeScript", "JavaScript", "Type Systems"],
    likes: 156,
    date: "2023-02-10",
    isLiked: false,
    isBookmarked: false,
    recommended: false
  },
  {
    id: "4",
    title: "Tailwind CSS Crash Course",
    description: "A quick introduction to Tailwind CSS with practical examples for rapid UI development.",
    url: "https://tailwindcss.com/docs",
    imageUrl: "https://images.pexels.com/photos/5483077/pexels-photo-5483077.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["CSS", "Tailwind", "Web Design"],
    likes: 132,
    date: "2023-04-05",
    isLiked: false,
    isBookmarked: false,
    recommended: false
  },
  {
    id: "5",
    title: "Modern JavaScript Tutorial",
    description: "From the basics to advanced topics with simple, but detailed explanations.",
    url: "https://javascript.info/",
    imageUrl: "https://images.pexels.com/photos/4709369/pexels-photo-4709369.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["JavaScript", "ES6", "Web Development"],
    likes: 208,
    date: "2023-01-30",
    isLiked: false,
    isBookmarked: false,
    recommended: true
  },
  {
    id: "6",
    title: "Web Performance Optimization",
    description: "Learn techniques to make your websites faster and more efficient for users.",
    url: "https://web.dev/performance-optimizing-content-efficiency/",
    imageUrl: "https://images.pexels.com/photos/7089401/pexels-photo-7089401.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
    tags: ["Performance", "Web Development", "Optimization"],
    likes: 94,
    date: "2023-05-12",
    isLiked: false,
    isBookmarked: false,
    recommended: false
  }
];