import { Module } from '@nestjs/common';
import { PrismaModule } from './prisma/prisma.module';
import { DishesModule } from './dishes/dishes.module';
import { ReviewsModule } from './reviews/reviews.module';
import { CategoriesModule } from './categories/categories.module';
import { UploadModule } from './upload/upload.module';

@Module({
  imports: [
    PrismaModule,
    DishesModule,
    ReviewsModule,
    CategoriesModule,
    UploadModule,
  ],
})
export class AppModule {}
