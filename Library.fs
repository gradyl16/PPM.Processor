// Library.fs
// Dylen Greenenwald
// dgree21
// 10/23/2023
//
// A library of F# ppm image processing functions. The functions take the dimensions of the
// image (width, height, depth) as arguments along with the image itself which is represented
// as a list of list of 3 tuples of ints. No looping, arrays, or mutable variables are utilized
// in their implementation.

namespace ImageLibrary

module Operations =
 
  let rec Grayscale (width:int) 
                    (height:int) 
                    (depth:int) 
                    (image:(int*int*int) list list) =
    // Convert a single pixel to grayscale
    let grayscalePixel (r, g, b) =
        let avg = (float r * 0.299) + (float g * 0.587) + (float b * 0.114)
        (int avg, int avg, int avg) // Make sure final pixel tuple contains ints

    // Convert a single row to grayscale
    let grayscaleRow row = List.map grayscalePixel row

    // Convert the entire image to grayscale
    let grayscaleImage = List.map grayscaleRow image

    grayscaleImage



  
  let rec Threshold (width:int) 
                    (height:int)
                    (depth:int)
                    (image:(int*int*int) list list)
                    (threshold:int) = 
    
    let thresholdPixel (r, g, b) = 
      let r = if r > threshold then 255 else 0
      let g = if g > threshold then 255 else 0
      let b = if b > threshold then 255 else 0
      (r, g, b)

    let thresholdRow row = List.map thresholdPixel row
    let thresholdImage = List.map thresholdRow image
  
    thresholdImage

  //
  // FlipHorizontal
  //
  // Takes the dimensions of the image and the image itself and returns an image
  // which is flipped along the vertical axis.
  //
  let rec FlipHorizontal (width:int)
                         (height:int)
                         (depth:int)
                         (image:(int*int*int) list list) = 
    match image with
    | [] -> []
    | head :: tail -> List.rev head :: FlipHorizontal width height depth tail


  let rec EdgeDetect (width:int)
               (height:int)
               (depth:int)
               (image:(int*int*int) list list)
               (threshold:int) = 
    let rec replaceRow (curRow: (int*int*int) list)
                       (lowerRow: (int*int*int) list) = 
      match curRow with
      | [lastPixel] -> []
      | _ ->
        // Get the relevant pixels
        let (r1, g1, b1) = List.head curRow
        let (r2, g2, b2) = List.head lowerRow
        let (r3, g3, b3) = List.item 1 curRow

        // Calculate color distances between current pixels and lower and righthand neighbors
        let distance1: float = sqrt ((float (r1 - r2) ** 2.0) + (float (g1 - g2) ** 2.0) + (float (b1 - b2) ** 2.0))
        let distance2: float = sqrt ((float (r1 - r3) ** 2.0) + (float (g1 - g3) ** 2.0) + (float (b1 - b3) ** 2.0))

        // Check if either distance crosses threshold
        if distance1 > float threshold || distance2 > float threshold then
          (0, 0, 0) :: replaceRow (List.tail curRow) (List.tail lowerRow)
        else
          (255, 255, 255) :: replaceRow (List.tail curRow) (List.tail lowerRow)

    match image with
    | [row] -> [] // If there is one row left, we know there is none below it
    | _ ->
      replaceRow (List.head image) (List.item 1 image) :: EdgeDetect width height depth (List.tail image) threshold


  let rec RotateRight90 (width:int)
                        (height:int)
                        (depth:int)
                        (image:(int*int*int) list list) =
    // Recursively rotating all columns rotates the entire image
    let rec rotateColumn i =
        // Base case: if i = width then we have rotated all columns
        if i = width then
            []
        // Recursive case: rotate the ith column and then rotate the next column
        else
            let rotatedColumn =
                // Select each row and then select the ith element of that row
                // Reverse the resulting list to avoid counterclockwise rotation
                List.map (fun row -> List.item i row) image |> List.rev
            rotatedColumn :: rotateColumn (i + 1)

    // Call the recursive function at the first column
    let rotatedImage = rotateColumn 0

    // Return the result
    rotatedImage