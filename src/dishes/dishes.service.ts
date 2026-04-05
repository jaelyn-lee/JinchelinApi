import { Injectable, NotFoundException } from '@nestjs/common';
import { Prisma } from '@prisma/client';
import { PrismaService } from '../prisma/prisma.service';

type DishWithRelations = Prisma.DishGetPayload<{
  include: { category: true; reviews: true };
}>;

@Injectable()
export class DishesService {
  constructor(private prisma: PrismaService) {}

  async findAll(search?: string, categoryId?: string, sortBy = 'rating') {
    const dishes = await this.prisma.dish.findMany({
      where: {
        ...(search && { name: { contains: search, mode: 'insensitive' } }),
        ...(categoryId && { category_id: categoryId }),
      },
      include: { category: true, reviews: true },
    });

    const mapped = dishes.map((d) => this.toResponse(d));

    return mapped.sort((a, b) => {
      if (sortBy === 'name') return a.name.localeCompare(b.name);
      if (sortBy === 'date')
        return (
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
      return b.avgRating - a.avgRating;
    });
  }

  async findOne(id: string) {
    const dish = await this.prisma.dish.findUnique({
      where: { id },
      include: { category: true, reviews: true },
    });
    if (!dish) throw new NotFoundException('Dish not found');
    return this.toResponse(dish);
  }

  async getRanking() {
    const dishes = await this.prisma.dish.findMany({
      where: { reviews: { some: {} } },
      include: { category: true, reviews: true },
    });

    return dishes
      .map((d) => this.toResponse(d))
      .sort(
        (a, b) => b.avgRating - a.avgRating || b.reviewCount - a.reviewCount,
      );
  }

  async create(data: { name: string; categoryId?: string }) {
    const dish = await this.prisma.dish.create({
      data: { name: data.name, category_id: data.categoryId ?? null },
      include: { category: true, reviews: true },
    });
    return this.toResponse(dish);
  }

  async remove(id: string) {
    const dish = await this.prisma.dish.findUnique({ where: { id } });
    if (!dish) throw new NotFoundException(`Dish not found for id: ${id}`);
    await this.prisma.dish.delete({ where: { id } });
  }

  private toResponse(dish: DishWithRelations) {
    const ratings = dish.reviews.map((r) => Number(r.rating));
    const avgRating = ratings.length
      ? Math.round(
          (ratings.reduce((a: number, b: number) => a + b, 0) /
            ratings.length) *
            10,
        ) / 10
      : 0;

    return {
      id: dish.id,
      name: dish.name,
      category: dish.category?.name ?? null,
      cuisine: dish.category?.cuisine ?? null,
      avgRating,
      reviewCount: dish.reviews.length,
      createdAt: dish.created_at,
    };
  }
}
