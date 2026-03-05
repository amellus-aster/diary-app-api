namespace DiaryApi.Models; 

public class DiaryModel
{
  public int Id {get; set;}
  public string UserId{get; set;} = default!; 
  public string Title{get; set;} = default!; 
  public string Content {get; set;}= default!; 
  public DateTime CreatedAt{get; set;}
  public DateTime UpdatedAt{get; set;}

}