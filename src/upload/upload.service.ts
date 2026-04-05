import { Injectable } from '@nestjs/common';
import { createClient } from '@supabase/supabase-js';

@Injectable()
export class UploadService {
  private supabase: ReturnType<typeof createClient>;

  constructor() {
    this.supabase = createClient(
      process.env.SUPABASE_URL!,
      process.env.SUPABASE_SERVICE_ROLE_KEY!,
    );
  }

  async uploadPhoto(file: Express.Multer.File): Promise<string> {
    const fileName = `${Date.now()}-${file.originalname}`;
    const bucket = process.env.SUPABASE_STORAGE_BUCKET ?? 'dish-photos';

    const { error } = await this.supabase.storage
      .from(bucket)
      .upload(fileName, file.buffer, { contentType: file.mimetype });

    if (error) throw new Error(error.message);

    const { data } = this.supabase.storage.from(bucket).getPublicUrl(fileName);

    return data.publicUrl;
  }
}
